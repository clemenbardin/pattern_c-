# Étape de build
FROM node:18-alpine AS build
WORKDIR /app

# Copie des fichiers de dépendances
COPY package*.json ./
RUN npm install

# Copie du reste et build
COPY . .
ARG VITE_API_URL=http://localhost:3000
ENV VITE_API_URL=$VITE_API_URL
RUN npm run build

# Étape de production avec Nginx
FROM nginx:alpine AS production
WORKDIR /usr/share/nginx/html

# Copie des fichiers construits depuis l'étape de build
COPY --from=build /app/dist .

# Copie de la configuration Nginx personnalisée
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 3000
CMD ["nginx", "-g", "daemon off;"]