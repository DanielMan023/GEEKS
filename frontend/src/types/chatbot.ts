// Tipos para el sistema de chatbot

export interface ChatbotMessage {
  message: string;
  userId?: number;
  sessionId?: string;
  context?: string;
}

export interface ChatbotResponse {
  message: string;
  type: 'text' | 'product_list' | 'quick_replies';
  quickReplies?: QuickReply[];
  productSuggestions?: ProductRecommendation[];
  intent?: string;
  confidence: number;
  metadata?: Record<string, any>;
}

export interface QuickReply {
  text: string;
  action: string;
  value?: string;
}

export interface ProductRecommendation {
  id: number;
  name: string;
  shortDescription?: string;
  price: number;
  discountPrice?: number;
  mainImage?: string;
  categoryName: string;
  brand?: string;
  relevanceScore: number;
  reason: string;
}

export interface ChatbotContext {
  recentSearches: string[];
  popularCategories: string[];
  trendingProducts: string[];
  userPreferences: Record<string, any>;
}

// Tipos para el estado del chat
export interface ChatMessage {
  id: string;
  text: string;
  isUser: boolean;
  timestamp: Date;
  type: 'text' | 'product_list' | 'quick_replies';
  quickReplies?: QuickReply[];
  productSuggestions?: ProductRecommendation[];
  intent?: string;
  confidence?: number;
}

export interface ChatbotState {
  messages: ChatMessage[];
  isOpen: boolean;
  isLoading: boolean;
  error: string | null;
  context: ChatbotContext | null;
}

// Tipos para las acciones del chatbot
export type ChatbotAction = 
  | { type: 'SEND_MESSAGE'; payload: string }
  | { type: 'RECEIVE_MESSAGE'; payload: ChatbotResponse }
  | { type: 'TOGGLE_CHAT' }
  | { type: 'SET_LOADING'; payload: boolean }
  | { type: 'SET_ERROR'; payload: string | null }
  | { type: 'SET_CONTEXT'; payload: ChatbotContext }
  | { type: 'CLEAR_CHAT' }
  | { type: 'QUICK_REPLY'; payload: QuickReply };
