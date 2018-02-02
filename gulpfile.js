// Dependencies
const {restore, test, build, publish, run} = require('gulp-dotnet-cli');
const argv = require('yargs').argv;
const download = require('gulp-download');
const decompress = require('gulp-decompress');
const path = require('path');
const mkpath = require('mkpath');
const gulp = require('gulp');
const http = require('http');
const fs = require('fs');
const del = require('del');
const cp = require('child_process');
const os = require('os');
const request = require('request');
const spawn = require('child-process-promise').spawn;
const exec = require('child-process-promise').exec;

// Context data
const branch = argv.environment || process.env.BRANCH_NAME || "unknown"; // your current git branch, you can override the current branch with the OVERRIDE_BRANCH env, or it will come from jenkins, finally it will just default to BRANCHUNKNOWN
//the environment we run in, calculated off the branch. You can replace this with your own strings, we will get the branch from jenkins.
//my kingdom for a regex, just replace some common characters that are bad in aws, and shorten release and feature
const environment = branch.replace('/', '-').replace('\\', '-').replace('origin-', '').replace('feature', 'ft').replace('release', 'rel');

// Dotnet context
const configuration = 'Release';
const version = '1.0.0';
const sourceDir = path.join(process.cwd(), 'src');
const outputDir = path.join(process.cwd(), 'output');
const publishOutputDir = path.join(outputDir, 'publishOutput');

// Terraform context
const terraformExe = 'terraform' + (os.platform() === 'win32' ? '.exe': ''); //executables in windows have an extension, unix exe's do not follow the same convention.
const terraformDir = path.join(__dirname, 'terraform');
const terraformPath = path.join(terraformDir, terraformExe);
const terraformVersion = '0.9.3';

//=============================USER CONFIGURATION, PLEASE FILL OUT ========================================================
//your aws region, default to us-east-1
const region = argv.region || 'us-east-1';
const app = 'NetCoreSample'; //your app's name, used to generate names for infrastructures

const terraformStateS3Bucket = '<TO_FILL | DOMAIN_NAME>-terraform-bucket'; // the s3 bucket where we will store our terraform files
const terraformAwsProfile = '<TO_FILL | DOMAIN_NAME>-terraform';

const mainProjectName = 'NetCoreSample.Service'; // The name of the main/entry project.

const containerRegistryUri = "<TO_FILL | ECR_URI>"; // URI to locate the container image registry
//=============================END USER CONFIGURATION ====================================================================

// Docker context
const dockerEnvironment = argv.dockerenv || environment;
const dockerTag = argv.dockertag || `${dockerEnvironment}-${version}`;
const dockerDir = path.join(sourceDir, 'Docker');
const dockerFilePath = path.join(sourceDir, mainProjectName, 'Dockerfile');

const entryAssemblyName = `${mainProjectName}.dll`;
const dockerImageTag = `${containerRegistryUri}:${dockerTag}`;


// ======================== 
// == Begin: Dot Net
// ========================

gulp.task('clean', () => del(['**/bin', '**/obj', 'output/*']));

gulp.task('restore', ['clean'], () => 
	gulp.src('${sourceDir}/*.sln')
		.pipe(restore())
);

gulp.task('build', ['restore'], () => 
	gulp.src('${sourceDir}/*.sln')
		.pipe(build(
			{
				configuration: configuration,
				version: version
			}
		))
);

gulp.task('test', ['build'], () =>
	gulp.src('${sourceDir}/**/*Tests.csproj')
		.pipe(test(
			{
				configuration: configuration,
				noBuild: true
			}
		))
);

gulp.task('publish', ['build'], () =>
	gulp.src(`${sourceDir}/${mainProjectName}`)
		.pipe(publish(
			{
				configuration: configuration,
				version: version,
				output: publishOutputDir
			}
		))
);

gulp.task('run', [], () => {
		if (argv.rebuild) {
			gulp.start('publish', () => {
					process.chdir(`${publishOutputDir}`);
					spawn('dotnet', [`${entryAssemblyName}`], { stdio: 'inherit' });
				}
			);
		} else {
			process.chdir(`${publishOutputDir}`);
			spawn('dotnet', [`${entryAssemblyName}`], { stdio: 'inherit' });
		}
	}
);

gulp.task('preflight', ['publish']);

// ======================== 
// == End: Dot Net
// ========================


// ======================== 
// == Begin: Docker
// ========================

gulp.task('docker:compile', ()=> 
    spawn('docker', ['build', '-t', dockerImageTag, '-f', dockerFilePath, sourceDir], {stdio:'inherit'})
);

gulp.task('docker:login', ()=>
    exec(`aws ecr --profile ${terraformAwsProfile} get-login --no-include-email --region ${region} `)
    .then((result)=>exec(result.stdout))
    
);
gulp.task('docker:push', ['docker:login', 'docker:compile'], ()=>
    spawn("docker", ["push", dockerImageTag], {stdio: 'inherit'})
    .then(()=>spawn('docker', ['rmi', dockerImageTag], {stdio:'inherit'}))
);

// ======================== 
// == End: Docker
// ========================


// ======================== 
// == Begin: Terraform
// ========================

gulp.task('terraform:download', ()=>{
    if(fs.existsSync(terraformPath)){
        return;
    } 
    var mappedOS;
    var arch = os.arch();
    var mappedArch;
    if(arch === 'ia32' || arch === 'x32' || arch === 'x86'){
        mappedArch = '386'
    }
    if(arch === 'x64'){
        mappedArch = 'amd64'
    }
    if(arch === 'arm'){
        mappedArch = 'arm'
    }
    if(process.platform == 'win32'){
        mappedOS = `windows`
    }
    if(process.platform === 'linux'){
        mappedOS = `linux`
    }
    // darwin === macos
    if(process.platform === 'darwin'){
        mappedOS = `darwin`
    }
    var url = `https://releases.hashicorp.com/terraform/${terraformVersion}/terraform_${terraformVersion}_${mappedOS}_${mappedArch}.zip`
    return download(url).pipe(decompress()).pipe(gulp.dest(terraformDir));

});

/* terraform tasks */
gulp.task('terraform:clean', ()=>del([path.join(terraformDir, '.terraform' )], {force: true}));
gulp.task('terraform:init', ['terraform:clean', 'terraform:download'], (cb)=>{
  exec(`${terraformPath} init -backend-config="bucket=${terraformStateS3Bucket}" -backend-config="key=${app}/${environment}/state.tf" -backend-config="region=${region}"`, {cwd:terraformDir})
  .then(a=>{
      console.log(a.stdout);
      console.log(a.stderr);
      cb();
  })
  .catch(a=>{
      console.log(a);
      cb(a);
    })
}
);

gulp.task('terraform:destroy', ['terraform:init'], ()=>
    spawn(terraformPath, ['destroy', '-force', 
		'-var', `environment=${environment}`,
    ], {stdio:'inherit', cwd:terraformDir})
);

gulp.task('terraform:plan', ['terraform:init'], ()=>
    spawn(terraformPath, ['get', '-update'], {stdio:'inherit', cwd:terraformDir})
    .then(a=>spawn(terraformPath,  ['plan',
		'-var', `environment=${environment}`,
  ], {stdio:'inherit', cwd:terraformDir}))
    
);

gulp.task('terraform:apply',['terraform:plan', 'terraform:init'], () =>
  spawn(terraformPath, ['apply', 
	'-var', `environment=${environment}`,
  ], {stdio:'inherit', cwd:terraformDir})
);

// ======================== 
// == End: Terraform
// ========================
