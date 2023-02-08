docker stop kf22023api-prod
docker docker container run --rm -it -d --name kf22023api-prod -p 18080:80 -p 18443:443 kf22023api-image
docker network connect kf22023-bridge kf22023api-prod