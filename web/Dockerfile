FROM node:12.13.0-alpine AS build
WORKDIR /app

COPY . ./
RUN npm install -g @angular/cli@8.3.9 && \
    npm install && \
    ng build --prod

FROM nginx:1.17.5-alpine
#COPY ./nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist/web/ /usr/share/nginx/html
