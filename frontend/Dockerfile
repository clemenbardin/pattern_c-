# Étape de build
FROM node:18-alpine AS build
WORKDIR /app

# Copie des fichiers de dépendances
COPY package*.json ./
RUN npm install

# Copie du reste et build
COPY . .
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