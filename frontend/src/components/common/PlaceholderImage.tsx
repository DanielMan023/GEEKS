import React from 'react';

interface PlaceholderImageProps {
  width: number;
  height: number;
  text?: string;
  className?: string;
}

const PlaceholderImage: React.FC<PlaceholderImageProps> = ({ 
  width, 
  height, 
  text = "No Image", 
  className = "" 
}) => {
  return (
    <svg
      width={width}
      height={height}
      viewBox={`0 0 ${width} ${height}`}
      className={className}
      xmlns="http://www.w3.org/2000/svg"
    >
      {/* Fondo */}
      <rect width={width} height={height} fill="#6B7280" />
      
      {/* Icono de imagen */}
      <g transform={`translate(${width/2 - 20}, ${height/2 - 25})`}>
        <rect x="0" y="0" width="40" height="30" fill="#9CA3AF" stroke="#E5E7EB" strokeWidth="2" rx="2" />
        <circle cx="8" cy="8" r="2" fill="#E5E7EB" />
        <polygon points="0,30 40,30 25,20 15,20 0,30" fill="#9CA3AF" />
      </g>
      
      {/* Texto */}
      <text
        x={width/2}
        y={height - 10}
        textAnchor="middle"
        fill="#FFFFFF"
        fontSize="12"
        fontFamily="Arial, sans-serif"
        fontWeight="bold"
      >
        {text}
      </text>
    </svg>
  );
};

export default PlaceholderImage;
