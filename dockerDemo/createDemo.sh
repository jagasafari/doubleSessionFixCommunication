#!/bin/bash
set -e
echo "################################################################################################################################"
mkdir demo
cd demo
git clone https://github.com/Azure-Samples/docker-flask-postgres.git
cd docker-flask-postgres
echo "################################################################################################################################"
sudo docker images
sudo docker build .
sudo docker images
sudo docker build -t docker-flask-sample .
sudo docker images
#vim Dockerfile
#vim requirements.txt
echo "################################################################################################################################"
#sudo docker run -it --env DBPASS=team123. --env DBHOST=dockerdemo.postgres.database.azure.com --env DBUSER=mika@dockerdemo --env DBNAME=postgres -p 5000:5000 docker-flask-sample
#az login
az postgres server list
#az postgres server create -n dockerDemo -g a -h
#az configure
#az configure -d group=a
#az configure -d location=westeurope
#az postgres server create -n dockerdemo -u mika -p team123. --sku-name B_Gen4_2
az postgres server list
az postgres server show -n dockerdemo --query fullyQualifiedDomainName
sudo docker run -it --env DBPASS=team123. --env DBHOST=dockerdemo.postgres.database.azure.com --env DBUSER=mika@dockerdemo --env DBNAME=postgres -p 5000:5000 docker-flask-sample
az postgres server update -n dockerdemo --ssl-enforcement Disabled
#az postgres server update --ssl-enforcement=Disabled
#az postgres server -n dockerdemoupdate --ssl-enforcment=Disabled
az postgres server list
