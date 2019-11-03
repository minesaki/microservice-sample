FROM trion/ng-cli:8.3.9 AS build
WORKDIR /app
USER root

COPY . ./
RUN ng build --prod

FROM nginx:1.17.5-alpine
#COPY ./nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist/web/ /usr/share/nginx/html
