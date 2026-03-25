# Build stage
FROM node:18 AS build
WORKDIR /app

# Copy package.json and package-lock.json
COPY package*.json ./
RUN npm install

# Copy everything else and build the production bundle
COPY . ./
RUN npm run build -- --configuration production

# Runtime stage (Nginx)
FROM nginx:alpine
COPY --from=build /app/dist/eventia-web/browser /usr/share/nginx/html

# Copy custom nginx configuration
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
