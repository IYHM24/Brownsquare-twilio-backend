#!/bin/bash

echo "🚀 Validando la plantilla de NestJS..."

# Verificar dependencias
echo "📦 Instalando dependencias..."
npm install

# Verificar linting
echo "🔍 Verificando calidad del código..."
npm run lint

# Verificar formateo
echo "✨ Formateando código..."
npm run format

# Ejecutar tests
echo "🧪 Ejecutando tests..."
npm run test

# Compilar proyecto
echo "🏗️ Compilando proyecto..."
npm run build

echo "✅ ¡Plantilla validada exitosamente!"
echo "🎉 Tu backend NestJS está listo para usar"