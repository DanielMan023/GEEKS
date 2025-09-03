const axios = require('axios');

async function testSimple() {
    try {
        console.log('🔍 Probando endpoint de salud...');
        const health = await axios.get('http://localhost:5000/api/Chatbot/health');
        console.log('✅ Salud:', health.data);
        
        console.log('\n🤖 Probando endpoint de IA...');
        const aiTest = await axios.post('http://localhost:5000/api/Chatbot/test-ai', {
            message: "Hola, ¿cómo estás?"
        });
        console.log('✅ Respuesta de IA:', aiTest.data);
        
    } catch (error) {
        console.log('❌ Error:', error.response?.status, error.response?.data || error.message);
    }
}

testSimple();
