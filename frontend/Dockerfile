# Stage 1: Build the React application
FROM node:20-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
ARG VITE_API_BASE
ENV VITE_API_BASE=$VITE_API_BASE
RUN npm run build

# Stage 2: Serve the build files with Nginx
FROM nginx:alpine
WORKDIR /app
# Copy the build output to Nginx's default public folder
COPY --from=build /app/dist /usr/share/nginx/html
# Copy custom Nginx config if you have one (essential for React Router)
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]