// Script para probar si ChatGPT está funcionando
const axios = require('axios');

const API_BASE_URL = 'http://localhost:5000/api/Chatbot';

async function testChatGPT() {
    console.log('🤖 Probando integración con ChatGPT...\n');
    
    try {
        // 1. Probar endpoint de salud
        console.log('1️⃣ Probando endpoint de salud...');
        const healthResponse = await axios.get(`${API_BASE_URL}/health`);
        console.log('✅ Salud del chatbot:', healthResponse.data);
        console.log('📋 Características disponibles:', healthResponse.data.features);
        console.log('');
        
        // 2. Probar endpoint de IA
        console.log('2️⃣ Probando integración con ChatGPT...');
        const testMessage = {
            message: "Hola, ¿cómo estás? Cuéntame sobre los productos de gaming que tienes."
        };
        
        const aiResponse = await axios.post(`${API_BASE_URL}/test-ai`, testMessage);
        console.log('✅ Respuesta de ChatGPT:', aiResponse.data);
        console.log('');
        
        // 3. Verificar si está funcionando
        if (aiResponse.data.aiWorking) {
            console.log('🎉 ¡CHATGPT ESTÁ FUNCIONANDO PERFECTAMENTE!');
            console.log('📝 Mensaje original:', aiResponse.data.originalMessage);
            console.log('🤖 Respuesta de IA:', aiResponse.data.aiResponse);
            console.log('🎯 Intención detectada:', aiResponse.data.intent);
            console.log('📊 Confianza:', aiResponse.data.confidence);
        } else {
            console.log('❌ ChatGPT no está funcionando');
            console.log('🔍 Error:', aiResponse.data.error);
        }
        
    } catch (error) {
        console.log('❌ Error probando ChatGPT:');
        if (error.response) {
            console.log('📡 Status:', error.response.status);
            console.log('📝 Respuesta:', error.response.data);
        } else {
            console.log('🌐 Error de conexión:', error.message);
        }
    }
}

// Ejecutar la prueba
testChatGPT();
