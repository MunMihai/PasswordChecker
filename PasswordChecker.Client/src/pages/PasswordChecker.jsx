import { useState, useEffect } from 'react';
import { apiService } from '../services/apiService';
import ProtectedRoute from '../components/ProtectedRoute';
import LimitExceededModal from '../components/LimitExceededModal';

function PasswordCheckerContent() {
  const [password, setPassword] = useState('');
  const [result, setResult] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [showLimitModal, setShowLimitModal] = useState(false);

  const handleCheck = async (e) => {
    e.preventDefault();
    if (!password) {
      setError('Please enter a password');
      return;
    }

    setError('');
    setLoading(true);
    setResult(null);

    try {
      const data = await apiService.checkPassword(password);
      setResult(data);
      setError('');
    } catch (err) {
      const errorMessage = err.response?.data?.message || err.response?.data || 'Failed to check password';
      setError(errorMessage);
      
      // Check if error is about limit exceeded
      if (errorMessage.includes('limit exceeded') || errorMessage.includes('Daily limit')) {
        setShowLimitModal(true);
      }
    } finally {
      setLoading(false);
    }
  };

  const getLevelColor = (level) => {
    switch (level) {
      case 'VERY_STRONG':
        return 'bg-green-500';
      case 'STRONG':
        return 'bg-green-400';
      case 'MEDIUM':
        return 'bg-yellow-400';
      case 'WEAK':
        return 'bg-orange-400';
      case 'VERY_WEAK':
        return 'bg-red-500';
      default:
        return 'bg-gray-400';
    }
  };

  const getLevelText = (level) => {
    return level.replace('_', ' ');
  };

  return (
    <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
      <LimitExceededModal 
        isOpen={showLimitModal}
        onClose={() => setShowLimitModal(false)}
        maxChecksPerDay={result?.maxChecksPerDay || 0}
      />
      <h1 className="text-3xl font-bold text-gray-900 mb-8">Password Checker</h1>

      <div className="bg-white shadow rounded-lg p-8">
        {result && result.remainingChecks !== null && result.maxChecksPerDay !== null && (
          <div className="mb-6 p-4 bg-blue-50 border border-blue-200 rounded-lg">
            <p className="text-sm text-gray-700">
              <span className="font-medium">Remaining checks today:</span>{' '}
              <span className="font-bold text-blue-600">{result.remainingChecks}</span> / {result.maxChecksPerDay}
            </p>
          </div>
        )}
        <form onSubmit={handleCheck} className="mb-6">
          <div className="mb-4">
            <label htmlFor="password" className="block text-sm font-medium text-gray-700 mb-2">
              Enter Password to Check
            </label>
            <input
              type="text"
              id="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              className="w-full px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="Enter your password"
            />
          </div>

          {error && (
            <div className="mb-4 bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded">
              {error}
            </div>
          )}

          <button
            type="submit"
            disabled={loading}
            className="w-full bg-blue-600 hover:bg-blue-700 text-white font-medium py-2 px-4 rounded-md disabled:opacity-50"
          >
            {loading ? 'Checking...' : 'Check Password'}
          </button>
        </form>

        {result && (
          <div className="mt-8 border-t pt-8">
            <div className="mb-6">
              <div className="flex items-center justify-between mb-2">
                <h3 className="text-lg font-semibold">Password Strength</h3>
                <span className={`px-3 py-1 rounded-full text-white text-sm font-medium ${getLevelColor(result.level)}`}>
                  {getLevelText(result.level)}
                </span>
              </div>
              <div className="w-full bg-gray-200 rounded-full h-4">
                <div
                  className={`h-4 rounded-full ${getLevelColor(result.level)}`}
                  style={{ width: `${result.score}%` }}
                ></div>
              </div>
              <p className="text-sm text-gray-600 mt-2">Score: {result.score}/100</p>
            </div>

            {result.recommendations && result.recommendations.length > 0 && (
              <div className="mb-6">
                <h3 className="text-lg font-semibold mb-3">Recommendations</h3>
                <ul className="list-disc list-inside space-y-2">
                  {result.recommendations.map((rec, index) => (
                    <li key={index} className="text-gray-700">{rec}</li>
                  ))}
                </ul>
              </div>
            )}

            <div className={`p-4 rounded-lg ${
              result.isValid ? 'bg-green-50 border border-green-200' : 'bg-red-50 border border-red-200'
            }`}>
              <p className={`font-medium ${result.isValid ? 'text-green-800' : 'text-red-800'}`}>
                {result.isValid
                  ? '✓ This password meets the minimum security requirements.'
                  : '✗ This password is too weak. Please follow the recommendations above.'}
              </p>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}

export default function PasswordChecker() {
  return (
    <ProtectedRoute>
      <PasswordCheckerContent />
    </ProtectedRoute>
  );
}
