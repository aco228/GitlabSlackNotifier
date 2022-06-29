:: go to the src folder

cd ..
cd src

set APP_NAME=[PLACEHOLDER_APP_NAME]
set HEROKU_API_KEY=[PLACEHOLDER_HEROKU_API_KEY]

echo 'Install dependencies`
dotnet restore -- force-evaluate

echo `Build`
dotnet build --configuration Release --no-restore

echo `Test`
echo `No tests`

echo `Copy docker file`
copy GitlabSlackNotifier.Api/Docker ./Docker


echo `Docker initialize`
docker login --username=_ --password=%HEROKU_API_KEY% registry.heroku.com

echo `Heroku push`
heroku container:push web -a %APP_NAME%
