docker network create -d bridge kf22023-bridge
docker stop kf22023rs-prod
docker container run -it -d --name kf22023rs-prod -v kf22023_data:/data -p 16379:6379 -p 18001:8001 redis/redis-stack:latest
docker network connect kf22023-bridge kf22023rs-prod
