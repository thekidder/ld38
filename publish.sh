set -e

# this script expects a number of environment variables to be set
# FORTRESS_SSH_SERVER   the address to push to
# FORTRESS_SSH_OPTIONS  any additional ssh options to use
# FORTRESS_PATH         the path on the server to publish files to

# example configuration:
# export FORTRESS_SSH_SERVER=user@example.com
# export FORTRESS_SSH_OPTIONS="-i <keyfile>"
# export FORTRESS_PATH=/var/www/FORTRESS

TOP_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

ssh $FORTRESS_SSH_OPTIONS $FORTRESS_SSH_SERVER "rm -rf $FORTRESS_PATH/*"
scp $FORTRESS_SSH_OPTIONS -r $TOP_DIR/Build/* $FORTRESS_SSH_SERVER:$FORTRESS_PATH/
