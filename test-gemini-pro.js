const axios = require('axios');

async function testGeminiPro() {
    try {
        console.log('🤖 Probando Gemini Pro en Google Cloud...\n');
        
        console.log('1️⃣ Probando endpoint de salud...');
        const health = await axios.get('http://localhost:5000/api/Chatbot/health');
        console.log('✅ Salud:', health.data.status);
        console.log('📋 Características:', health.data.features);
        console.log('');
        
        console.log('2️⃣ Probando Gemini Pro directamente...');
        const aiTest = await axios.post('http://localhost:5000/api/Chatbot/test-openai-direct', {
            message: "Hola, ¿cómo estás? Cuéntame sobre los productos de gaming que tienes."
        });
        
        console.log('✅ Respuesta completa:', JSON.stringify(aiTest.data, null, 2));
        console.log('');
        
        if (aiTest.data.aiWorking) {
            console.log('🎉 ¡GEMINI PRO ESTÁ FUNCIONANDO!');
            console.log('📝 Mensaje original:', aiTest.data.originalMessage);
            console.log('🤖 Respuesta de Gemini Pro:', aiTest.data.aiResponse);
            console.log('🔧 Tipo de prueba:', aiTest.data.testType);
            console.log('');
            console.log('🚀 ¡Tu chatbot ahora usa IA de Google Cloud!');
        } else {
            console.log('❌ Gemini Pro no está funcionando');
            console.log('🔍 Error:', aiTest.data.error);
            console.log('📋 Detalles:', aiTest.data.details);
        }
        
    } catch (error) {
        console.log('❌ Error probando Gemini Pro:');
        if (error.response) {
            console.log('📡 Status:', error.response.status);
            console.log('📝 Respuesta:', error.response.data);
        } else {
            console.log('🌐 Error de conexión:', error.message);
        }
    }
}

testGeminiPro();
