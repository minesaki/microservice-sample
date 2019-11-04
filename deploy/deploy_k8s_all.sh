#!/bin/bash

########################
# mandatory components #
########################

## RabbitMQ
kubectl apply -f ./kubernetes/RabbitMQ.yaml

## ingress-controller
## get ingress-controller manifests
## curl https://raw.githubusercontent.com/kubernetes/ingress-nginx/nginx-0.26.1/deploy/static/mandatory.yaml > ./kubernetes/ingress-controller/mandatory.yaml
## curl https://raw.githubusercontent.com/kubernetes/ingress-nginx/nginx-0.26.1/deploy/static/provider/cloud-generic.yaml > ./kubernetes/ingress-controller/cloud-generic.yaml
kubectl apply -f ./kubernetes/ingress-controller/mandatory.yaml
kubectl apply -f ./kubernetes/ingress-controller/cloud-generic.yaml

## Ingress service
kubectl apply -f ./Kubernetes/Ingress.yaml

###########################
# build & deploy services #
###########################

# log-service
docker build -t log-service:1.0.0 -f ../services/log-service/Dockerfile ../services/log-service
kubectl apply -f ./kubernetes/LogService.yaml

# omikuji-service
docker build -t omikuji-service:1.0.0 -f ../services/omikuji-service/Dockerfile ../services/omikuji-service
kubectl apply -f ./kubernetes/OmikujiService.yaml

# api-service
docker build -t api-service:1.0.0 -f ../services/api-service/Dockerfile ../services/api-service
kubectl apply -f ./kubernetes/ApiService.yaml

# web
docker build -t web:1.0.0 -f ../web/Dockerfile ../web
kubectl apply -f ./kubernetes/Web.yaml
