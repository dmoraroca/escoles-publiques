*** Per aixecar postgress? ***

1 => docker compose up -d
2 => docker ps

o per vsCode amb el menu contextual Start

*** Aturar PostgreSQL ***

docker compose down

*** Reiniciar PostgreSQL ***

docker compose restart

*** Aixecar PostgreSQL ***


docker start escoles-db

*** Veure logs PostgreSQL ***

docker logs escoles-db

*** Comprova estat ***
docker ps

*** Recomanació pràctica ***

Per ús diari → VS Code (més còmode).
Per scripts / CI → docker compose up -d.

*** scaffold ***

dotnet ef dbcontext scaffold \
"Host=localhost;Port=5432;Database=escoles;Username=app;Password=app" \
Npgsql.EntityFrameworkCore.PostgreSQL \
--project src/Infrastructure \
--output-dir Persistence/Scaffold \
--schema public \
--no-context \
--force

*** Git create a new repository on the command line ***

echo "# escoles-publiques" >> README.md
git init
git add README.md
git commit -m "first commit"
git branch -M main
git remote add origin https://github.com/dmoraroca/escoles-publiques.git
git push -u origin main

*** Git push an existing repository from the command line ***

git remote add origin https://github.com/dmoraroca/escoles-publiques.git
git branch -M main
git push -u origin main