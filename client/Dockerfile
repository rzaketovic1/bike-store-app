# Build stage
FROM node:20 AS build
WORKDIR /app

COPY . .
RUN npm install
RUN npm run build --configuration=production

# Nginx stage
FROM nginx:alpine
COPY --from=build /app/dist/client /usr/share/nginx/html

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]