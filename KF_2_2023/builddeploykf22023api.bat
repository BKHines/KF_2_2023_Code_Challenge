docker network create -d bridge kf22023-bridge
docker stop kf22023api-prod
docker rm kf22023api-prod
docker build --rm -t kf22023api-image -f Dockerfile . 
docker container run -v C:/ProgramData/certify/assets/kf22023api.duckdns.org:/https -it -d --name kf22023api-prod -p 18080:80 -p 18443:443 -e ASPNETCORE_URLS="https://+;http://+" -e ASPNETCORE_HTTPS_PORT=18443 -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/20230509_6e02e81a.pfx kf22023api-image
docker network connect kf22023-bridge kf22023api-prod