#!/bin/bash
set -e

CERT_DIR="/certs"
mkdir -p $CERT_DIR

echo "Generating development certificates..."
dotnet dev-certs https -ep $CERT_DIR/aspnetapp.pfx -p password

echo "Converting certificates for Vite..."
openssl pkcs12 -in $CERT_DIR/aspnetapp.pfx -nocerts -out $CERT_DIR/key.pem -password pass:password -nodes
openssl pkcs12 -in $CERT_DIR/aspnetapp.pfx -clcerts -nokeys -out $CERT_DIR/cert.pem -password pass:password

echo "Setting proper permissions..."
# Make files readable by all users in the container
chmod 644 $CERT_DIR/cert.pem
chmod 644 $CERT_DIR/aspnetapp.pfx  # Make PFX readable
chmod 600 $CERT_DIR/key.pem

# Ensure any user can access the directory
chmod 755 $CERT_DIR

echo "Certificate generation complete!"