FROM node:20-alpine

WORKDIR /app

# Copy package files
COPY package*.json ./
RUN npm ci

# Copy project files
COPY . .

# Expose the port Vite uses
EXPOSE 7040

# Command to start Vite dev server with debug output
CMD ["sh", "-c", "echo 'Starting Vite server...' && npm run dev -- --host 0.0.0.0 --port 7040"]