#!/bin/bash

###################
# delete services #
###################

# web
kubectl delete -f ./kubernetes/Web.yaml

# api-service
kubectl delete -f ./kubernetes/ApiService.yaml

# omikuji-service
kubectl delete -f ./kubernetes/OmikujiService.yaml

# log-service
kubectl delete -f ./kubernetes/LogService.yaml


########################
# mandatory components #
########################

## Ingress service
kubectl delete -f ./Kubernetes/Ingress.yaml

## ingress-controller
kubectl delete -f ./kubernetes/ingress-controller/cloud-generic.yaml
kubectl delete -f ./kubernetes/ingress-controller/mandatory.yaml

## RabbitMQ
kubectl delete -f ./kubernetes/RabbitMQ.yaml
