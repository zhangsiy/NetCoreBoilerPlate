FROM zhangsiy/linux-dev:0.1.0
# add package folder to container
ADD . ./app
# set up working directory
WORKDIR /app
# Reinstall all npm modules including local gulp
RUN npm install \
	&& npm install bower -g
# Do a clean up from the context
# Note: This is mainly targeted at cleaning up the output folder to allow
#       mounting. We can do this with dockerignore file, but that would 
#       interfere with the bundling image, as docker does not yet support
#       multiple dockerignore files...
RUN gulp clean
