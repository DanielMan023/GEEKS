import React, { useState, useEffect, useRef } from 'react';
import { MessageCircle, X, Send, Bot, User, ShoppingBag, TrendingUp, Search } from 'lucide-react';
import ReactMarkdown from 'react-markdown';
import { chatbotService } from '../../services/chatbotService';
import { ChatMessage, ChatbotResponse, QuickReply, ProductRecommendation } from '../../types/chatbot';
import { fileService } from '../../services/fileService';
import PlaceholderImage from '../common/PlaceholderImage';

interface ChatbotWidgetProps {
  className?: string;
}

const ChatbotWidget: React.FC<ChatbotWidgetProps> = ({ className = '' }) => {
  const [isOpen, setIsOpen] = useState(false);
  const [messages, setMessages] = useState<ChatMessage[]>([]);
  const [inputMessage, setInputMessage] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const messagesEndRef = useRef<HTMLDivElement>(null);

  // Mensaje de bienvenida inicial
  useEffect(() => {
    if (messages.length === 0) {
      addWelcomeMessage();
    }
  }, []);

  // Auto-scroll al último mensaje
  useEffect(() => {
    scrollToBottom();
  }, [messages]);

  const addWelcomeMessage = () => {
          const welcomeMessage: ChatMessage = {
        id: 'welcome',
        text: '¡Hola! Soy tu asistente virtual de GEEKS con IA avanzada. ¿En qué puedo ayudarte hoy? 🤖✨',
        isUser: false,
        timestamp: new Date(),
        type: 'text',
        quickReplies: [
          { text: '🔍 Buscar productos', action: 'search_products' },
          { text: '📂 Ver categorías', action: 'view_categories' },
          { text: '❓ Necesito ayuda', action: 'help' }
        ]
      };
    setMessages([welcomeMessage]);
  };

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  };

  const handleSendMessage = async (message: string) => {
    if (!message.trim() || isLoading) return;

    // Agregar mensaje del usuario
    const userMessage: ChatMessage = {
      id: Date.now().toString(),
      text: message,
      isUser: true,
      timestamp: new Date(),
      type: 'text'
    };

    setMessages(prev => [...prev, userMessage]);
    setInputMessage('');
    setIsLoading(true);
    setError(null);

    try {
      // Enviar mensaje al chatbot
      const response = await chatbotService.sendMessage(message);
      
      // Agregar respuesta del chatbot
      const botMessage: ChatMessage = {
        id: (Date.now() + 1).toString(),
        text: response.message,
        isUser: false,
        timestamp: new Date(),
        type: response.type,
        quickReplies: response.quickReplies,
        productSuggestions: response.productSuggestions,
        intent: response.intent,
        confidence: response.confidence
      };

      setMessages(prev => [...prev, botMessage]);
    } catch (err: any) {
      setError(err.message);
      
      // Agregar mensaje de error
      const errorMessage: ChatMessage = {
        id: (Date.now() + 1).toString(),
        text: 'Lo siento, estoy teniendo problemas técnicos. ¿Puedes intentar de nuevo?',
        isUser: false,
        timestamp: new Date(),
        type: 'text'
      };

      setMessages(prev => [...prev, errorMessage]);
    } finally {
      setIsLoading(false);
    }
  };

  const handleQuickReply = (quickReply: QuickReply) => {
    handleSendMessage(quickReply.text);
  };

  const handleProductClick = (product: ProductRecommendation) => {
    // Aquí podrías navegar al producto o abrir un modal
    console.log('Producto clickeado:', product);
    // Por ahora solo mostramos en consola
    alert(`Producto: ${product.name}\nPrecio: ${formatPrice(product.price)}`);
  };

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat('es-CO', {
      style: 'currency',
      currency: 'COP',
      minimumFractionDigits: 0
    }).format(price);
  };

  const toggleChat = () => {
    setIsOpen(!isOpen);
    if (!isOpen && messages.length === 0) {
      addWelcomeMessage();
    }
  };

  return (
    <div className={`fixed bottom-4 right-4 z-50 ${className}`}>
      {/* Botón flotante del chatbot */}
      {!isOpen && (
                  <button
            onClick={toggleChat}
            className="bg-gradient-to-r from-green-600 to-blue-600 hover:from-green-700 hover:to-blue-700 text-white p-4 rounded-full shadow-lg transition-all duration-300 hover:scale-110"
            title="Abrir chat con asistente virtual con IA"
          >
            <div className="relative">
              <MessageCircle size={24} />
              <div className="absolute -top-1 -right-1 w-3 h-3 bg-yellow-400 rounded-full animate-pulse"></div>
            </div>
          </button>
      )}

      {/* Ventana del chat */}
      {isOpen && (
        <div className="bg-white rounded-lg shadow-2xl w-96 h-[500px] flex flex-col border border-gray-200">
          {/* Header */}
                      <div className="bg-gradient-to-r from-green-600 to-blue-600 text-white p-4 rounded-t-lg flex items-center justify-between">
              <div className="flex items-center gap-2">
                <div className="relative">
                  <Bot size={20} />
                  <div className="absolute -top-1 -right-1 w-2 h-2 bg-yellow-400 rounded-full animate-pulse"></div>
                </div>
                <div>
                  <div className="font-semibold">Asistente IA GEEKS</div>
                  <div className="text-xs opacity-90">Powered by Gemini Pro</div>
                </div>
              </div>
            <button
              onClick={toggleChat}
              className="text-white hover:text-gray-200 transition-colors"
            >
              <X size={20} />
            </button>
          </div>

          {/* Mensajes */}
          <div className="flex-1 overflow-y-auto p-4 space-y-4">
            {messages.map((message) => (
              <div
                key={message.id}
                className={`flex ${message.isUser ? 'justify-end' : 'justify-start'}`}
              >
                <div
                  className={`max-w-[80%] rounded-lg p-3 ${
                    message.isUser
                      ? 'bg-green-600 text-white'
                      : 'bg-gray-100 text-gray-800'
                  }`}
                >
                  {/* Mensaje de texto con markdown */}
                  <div className={`prose prose-sm max-w-none ${
                    message.isUser 
                      ? 'prose-invert' 
                      : 'prose-gray'
                  }`}>
                    <ReactMarkdown
                      components={{
                        table: ({ children }) => (
                          <div className="overflow-x-auto my-4">
                            <table className="w-full border-collapse border border-gray-300 rounded-lg overflow-hidden">
                              {children}
                            </table>
                          </div>
                        ),
                        thead: ({ children }) => (
                          <thead className="bg-gray-50">
                            {children}
                          </thead>
                        ),
                        tbody: ({ children }) => (
                          <tbody className="bg-white divide-y divide-gray-200">
                            {children}
                          </tbody>
                        ),
                        th: ({ children }) => (
                          <th className="border border-gray-300 bg-gray-100 px-4 py-3 text-left font-semibold text-gray-900 text-sm">
                            {children}
                          </th>
                        ),
                        td: ({ children }) => (
                          <td className="border border-gray-300 px-4 py-3 text-sm text-gray-700">
                            {children}
                          </td>
                        ),
                        tr: ({ children }) => (
                          <tr className="hover:bg-gray-50 transition-colors">
                            {children}
                          </tr>
                        ),
                        strong: ({ children }) => (
                          <strong className="font-bold text-gray-900">
                            {children}
                          </strong>
                        ),
                        code: ({ children }) => (
                          <code className="bg-gray-100 px-1 py-0.5 rounded text-sm font-mono">
                            {children}
                          </code>
                        ),
                      }}
                    >
                      {message.text}
                    </ReactMarkdown>
                  </div>
                  
                  {/* Respuestas rápidas */}
                  {message.quickReplies && !message.isUser && (
                    <div className="mt-3 space-y-2">
                      {message.quickReplies.map((reply, index) => (
                        <button
                          key={index}
                          onClick={() => handleQuickReply(reply)}
                          className="block w-full text-left px-3 py-2 bg-white bg-opacity-20 rounded text-sm hover:bg-opacity-30 transition-colors"
                        >
                          {reply.text}
                        </button>
                      ))}
                    </div>
                  )}

                  {/* Sugerencias de productos */}
                  {message.productSuggestions && !message.isUser && (
                    <div className="mt-3 space-y-2">
                      <div className="text-xs text-gray-600 mb-2">Productos recomendados:</div>
                      {message.productSuggestions.map((product) => (
                        <div
                          key={product.id}
                          onClick={() => handleProductClick(product)}
                          className="flex items-center gap-3 p-2 bg-white bg-opacity-20 rounded cursor-pointer hover:bg-opacity-30 transition-colors"
                        >
                          <div className="w-12 h-12 flex-shrink-0">
                            {product.mainImage ? (
                              <img
                                src={fileService.getImageUrl(product.mainImage)}
                                alt={product.name}
                                className="w-12 h-12 rounded object-cover"
                                onError={(e) => {
                                  const target = e.target as HTMLImageElement;
                                  target.style.display = 'none';
                                  target.nextElementSibling?.classList.remove('hidden');
                                }}
                              />
                            ) : (
                              <PlaceholderImage
                                width={48}
                                height={48}
                                text="No Image"
                                className="w-12 h-12 rounded"
                              />
                            )}
                            <PlaceholderImage
                              width={48}
                              height={48}
                              text="No Image"
                              className="w-12 h-12 rounded hidden"
                            />
                          </div>
                          <div className="flex-1 min-w-0">
                            <div className="font-medium text-sm truncate">{product.name}</div>
                            <div className="text-xs opacity-80">{product.categoryName}</div>
                            <div className="font-bold">{formatPrice(product.price)}</div>
                          </div>
                        </div>
                      ))}
                    </div>
                  )}

                  {/* Indicador de confianza */}
                  {message.confidence !== undefined && !message.isUser && (
                    <div className="mt-2 text-xs opacity-60">
                      Confianza: {Math.round(message.confidence * 100)}%
                    </div>
                  )}
                </div>
              </div>
            ))}

            {/* Indicador de carga */}
            {isLoading && (
              <div className="flex justify-start">
                <div className="bg-gray-100 rounded-lg p-3">
                  <div className="flex items-center gap-2">
                    <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-green-600"></div>
                    <span className="text-sm text-gray-600">Escribiendo...</span>
                  </div>
                </div>
              </div>
            )}

            {/* Error */}
            {error && (
              <div className="flex justify-start">
                <div className="bg-red-100 border border-red-300 text-red-700 rounded-lg p-3">
                  <div className="text-sm">{error}</div>
                </div>
              </div>
            )}

            <div ref={messagesEndRef} />
          </div>

          {/* Input */}
          <div className="p-4 border-t border-gray-200">
            <div className="flex gap-2">
              <input
                type="text"
                value={inputMessage}
                onChange={(e) => setInputMessage(e.target.value)}
                onKeyPress={(e) => e.key === 'Enter' && handleSendMessage(inputMessage)}
                placeholder="Escribe tu mensaje..."
                className="flex-1 px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
                disabled={isLoading}
              />
              <button
                onClick={() => handleSendMessage(inputMessage)}
                disabled={!inputMessage.trim() || isLoading}
                className="px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
              >
                <Send size={16} />
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default ChatbotWidget;
