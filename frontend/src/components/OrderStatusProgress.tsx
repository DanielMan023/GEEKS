import React from 'react';
import { CheckCircle, Clock, Truck, Package } from 'lucide-react';

interface OrderStatusProgressProps {
  status: string;
  className?: string;
}

const OrderStatusProgress: React.FC<OrderStatusProgressProps> = ({ status, className = '' }) => {
  const steps = [
    { key: 'Pending', label: 'Pendiente', icon: Clock },
    { key: 'Processing', label: 'Procesando', icon: Package },
    { key: 'Shipped', label: 'Enviado', icon: Truck },
    { key: 'Delivered', label: 'Entregado', icon: CheckCircle }
  ];

  const getCurrentStepIndex = () => {
    switch (status) {
      case 'Pending': return 0;
      case 'Processing': return 1;
      case 'Shipped': return 2;
      case 'Delivered': return 3;
      default: return 0;
    }
  };

  const currentStepIndex = getCurrentStepIndex();


  return (
    <div className={`w-full ${className}`}>
      <div className="flex items-center justify-between">
        {steps.map((step, index) => {
          const Icon = step.icon;
          const isCompleted = index <= currentStepIndex;
          const isCurrent = index === currentStepIndex;
          
          return (
            <div key={step.key} className="flex flex-col items-center">
              <div className={`
                w-10 h-10 rounded-full flex items-center justify-center border-2 transition-colors
                ${isCompleted 
                  ? 'bg-green-500 border-green-500 text-white' 
                  : isCurrent
                  ? 'bg-blue-500 border-blue-500 text-white'
                  : 'bg-gray-100 border-gray-300 text-gray-400'
                }
              `}>
                <Icon className="w-5 h-5" />
              </div>
              <span className={`
                mt-2 text-xs font-medium text-center
                ${isCompleted || isCurrent ? 'text-gray-900' : 'text-gray-400'}
              `}>
                {step.label}
              </span>
              {index < steps.length - 1 && (
                <div className={`
                  absolute top-5 left-1/2 w-full h-0.5 -z-10
                  ${isCompleted ? 'bg-green-500' : 'bg-gray-200'}
                `} style={{ width: 'calc(100% - 2.5rem)', marginLeft: '1.25rem' }} />
              )}
            </div>
          );
        })}
      </div>
    </div>
  );
};

export default OrderStatusProgress;
