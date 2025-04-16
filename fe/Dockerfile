# Base image
FROM node:18-alpine

# Set working directory 
WORKDIR /app

# Copy package files 
COPY package*.json ./

# Install dependencies
RUN npm install --force

# Copy the rest of the project 
COPY . .

# Build app (Next.js sẽ tạo .next folder) 
RUN npm run build

# Expose port 3000 
EXPOSE 3000

# Start the app 
CMD ["npm","start"]