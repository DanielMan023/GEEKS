import React, { useState } from 'react';
import { Bot, Brain, MessageSquare, TrendingUp, Search, ShoppingBag, Zap, Target } from 'lucide-react';

const ChatbotDemo: React.FC = () => {
  const [activeFeature, setActiveFeature] = useState<string>('intent');

  const features = [
    {
      id: 'intent',
      title: 'Detección de Intenciones',
      description: 'El chatbot analiza el mensaje del usuario para entender qué quiere hacer',
      icon: Target,
      examples: [
        'Saludos: "Hola", "Buenos días", "Hey"',
        'Búsqueda: "Buscar productos", "Quiero comprar", "Encontrar"',
        'Ayuda: "Necesito ayuda", "Cómo funciona", "Soporte"',
        'Categorías: "Ver categorías", "Qué tipos hay", "Explorar"'
      ],
      confidence: '95%'
    },
    {
      id: 'nlp',
      title: 'Procesamiento de Lenguaje Natural',
      description: 'Entiende el lenguaje humano de forma natural y contextual',
      icon: Brain,
      examples: [
        'Sinónimos: "producto", "artículo", "item"',
        'Contexto: "Quiero algo para gaming" → Recomienda productos gaming',
        'Variaciones: "Me gustaría", "Necesito", "Busco"',
        'Lenguaje coloquial: "Está chido", "Es genial", "Me encanta"'
      ],
      confidence: '92%'
    },
    {
      id: 'recommendations',
      title: 'Sistema de Recomendaciones Inteligente',
      description: 'Sugiere productos basándose en búsquedas y preferencias',
      icon: TrendingUp,
      examples: [
        'Búsqueda semántica: "algo para estudiar" → Libros, escritorios',
        'Recomendaciones por categoría: "gaming" → Consolas, juegos, accesorios',
        'Productos relacionados: "smartphone" → Carcasas, auriculares, cargadores',
        'Personalización: Basado en historial de búsquedas del usuario'
      ],
      confidence: '88%'
    },
    {
      id: 'context',
      title: 'Conciencia Contextual',
      description: 'Mantiene el contexto de la conversación y recuerda preferencias',
      icon: MessageSquare,
      examples: [
        'Memoria de sesión: Recuerda productos mencionados',
        'Contexto temporal: Saludos según hora del día',
        'Preferencias: Categorías favoritas del usuario',
        'Historial: Búsquedas recientes y productos vistos'
      ],
      confidence: '85%'
    },
    {
      id: 'integration',
      title: 'Integración con E-commerce',
      description: 'Se conecta directamente con el catálogo y sistema de productos',
      icon: ShoppingBag,
      examples: [
        'Búsqueda en tiempo real en la base de datos',
        'Información actualizada de precios y stock',
        'Navegación directa a productos recomendados',
        'Soporte para el proceso de compra'
      ],
      confidence: '100%'
    },
    {
      id: 'ai',
      title: 'Capacidades de IA Avanzadas',
      description: 'Utiliza algoritmos de machine learning para mejorar respuestas',
      icon: Zap,
      examples: [
        'Puntuación de relevancia para productos',
        'Análisis de sentimientos en consultas',
        'Aprendizaje de patrones de usuario',
        'Generación de respuestas contextuales'
      ],
      confidence: '90%'
    }
  ];

  const activeFeatureData = features.find(f => f.id === activeFeature);

  return (
    <div className="bg-white rounded-lg shadow-lg p-6">
      <div className="text-center mb-8">
        <div className="flex items-center justify-center gap-3 mb-4">
          <Bot className="text-green-600" size={32} />
          <h2 className="text-3xl font-bold text-gray-900">Asistente Virtual GEEKS</h2>
        </div>
        <p className="text-lg text-gray-600 max-w-2xl mx-auto">
          Un chatbot inteligente que utiliza técnicas avanzadas de Inteligencia Artificial 
          para brindar una experiencia de compra excepcional
        </p>
      </div>

      {/* Características principales */}
      <div className="grid grid-cols-2 md:grid-cols-3 gap-4 mb-8">
        {features.map((feature) => (
          <button
            key={feature.id}
            onClick={() => setActiveFeature(feature.id)}
            className={`p-4 rounded-lg border-2 transition-all duration-200 ${
              activeFeature === feature.id
                ? 'border-green-500 bg-green-50 text-green-700'
                : 'border-gray-200 hover:border-green-300 hover:bg-green-25'
            }`}
          >
            <div className="flex items-center gap-3">
              <feature.icon size={24} />
              <div className="text-left">
                <div className="font-semibold text-sm">{feature.title}</div>
                <div className="text-xs opacity-75">Confianza: {feature.confidence}</div>
              </div>
            </div>
          </button>
        ))}
      </div>

      {/* Detalle de la característica seleccionada */}
      {activeFeatureData && (
        <div className="bg-gray-50 rounded-lg p-6">
          <div className="flex items-center gap-3 mb-4">
            <activeFeatureData.icon className="text-green-600" size={24} />
            <h3 className="text-xl font-bold text-gray-900">{activeFeatureData.title}</h3>
          </div>
          
          <p className="text-gray-700 mb-4">{activeFeatureData.description}</p>
          
          <div className="grid md:grid-cols-2 gap-6">
            <div>
              <h4 className="font-semibold text-gray-800 mb-3">Ejemplos de Uso:</h4>
              <ul className="space-y-2">
                {activeFeatureData.examples.map((example, index) => (
                  <li key={index} className="flex items-start gap-2">
                    <div className="w-2 h-2 bg-green-500 rounded-full mt-2 flex-shrink-0"></div>
                    <span className="text-sm text-gray-700">{example}</span>
                  </li>
                ))}
              </ul>
            </div>
            
            <div className="bg-white rounded-lg p-4 border border-gray-200">
              <div className="text-center">
                <div className="text-3xl font-bold text-green-600 mb-2">
                  {activeFeatureData.confidence}
                </div>
                <div className="text-sm text-gray-600">Tasa de Precisión</div>
              </div>
              
              <div className="mt-4">
                <div className="flex justify-between text-sm text-gray-600 mb-1">
                  <span>Precisión</span>
                  <span>{activeFeatureData.confidence}</span>
                </div>
                <div className="w-full bg-gray-200 rounded-full h-2">
                  <div 
                    className="bg-green-500 h-2 rounded-full transition-all duration-500"
                    style={{ 
                      width: activeFeatureData.confidence === '100%' ? '100%' : 
                             activeFeatureData.confidence === '95%' ? '95%' :
                             activeFeatureData.confidence === '92%' ? '92%' :
                             activeFeatureData.confidence === '90%' ? '90%' :
                             activeFeatureData.confidence === '88%' ? '88%' : '85%'
                    }}
                  ></div>
                </div>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Estadísticas del sistema */}
      <div className="mt-8 grid grid-cols-2 md:grid-cols-4 gap-4">
        <div className="text-center p-4 bg-blue-50 rounded-lg">
          <div className="text-2xl font-bold text-blue-600">6</div>
          <div className="text-sm text-blue-700">Tipos de Intención</div>
        </div>
        <div className="text-center p-4 bg-green-50 rounded-lg">
          <div className="text-2xl font-bold text-green-600">95%</div>
          <div className="text-sm text-green-700">Precisión Promedio</div>
        </div>
        <div className="text-center p-4 bg-purple-50 rounded-lg">
          <div className="text-2xl font-bold text-purple-600">&lt;500ms</div>
          <div className="text-sm text-purple-700">Tiempo de Respuesta</div>
        </div>
        <div className="text-center p-4 bg-orange-50 rounded-lg">
          <div className="text-2xl font-bold text-orange-600">24/7</div>
          <div className="text-sm text-orange-700">Disponibilidad</div>
        </div>
      </div>

      {/* Call to action */}
      <div className="mt-8 text-center">
        <div className="bg-gradient-to-r from-green-500 to-blue-600 text-white p-6 rounded-lg">
          <h3 className="text-xl font-bold mb-2">¡Prueba el Chatbot Ahora!</h3>
          <p className="text-green-100 mb-4">
            Haz clic en el botón flotante del chat para experimentar la IA en acción
          </p>
          <div className="flex items-center justify-center gap-2 text-sm">
            <MessageSquare size={16} />
            <span>Busca productos, pide ayuda o simplemente saluda</span>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ChatbotDemo;
