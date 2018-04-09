#!/bin/bash
set -e
sudo rm demo -r
sudo docker rmi python
sudo docker rmi docker-flask-sample
az postgres server delete -n dockerdemo -y
