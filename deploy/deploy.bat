:: go to the src folder

cd ..
cd src

set APP_NAME=gitlab-slack-notifier
set HEROKU_API_KEY=7aa177b8-e4ac-4601-97b0-884a1a8b3793

echo `Heroku deploy`
heroku container:release web -a %APP_NAME%