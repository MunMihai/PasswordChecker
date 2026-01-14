import { createContext, useContext, useState, useEffect } from 'react';
import { authService } from '../services/authService';

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const token = authService.getToken();
    const userData = authService.getUser();
    if (token && userData) {
      setUser(userData);
    }
    setLoading(false);
  }, []);

  const login = async (email, password) => {
    try {
      const response = await authService.login(email, password);
      const userData = {
        id: response.userId,
        email: response.email,
        role: response.role,
      };
      authService.setAuth(response.token, userData);
      setUser(userData);
      return response;
    } catch (error) {
      throw error;
    }
  };

  const register = async (email, password, varsta, gen) => {
    try {
      const response = await authService.register(email, password, varsta, gen);
      const userData = {
        id: response.userId,
        email: response.email,
        role: response.role,
      };
      authService.setAuth(response.token, userData);
      setUser(userData);
      return response;
    } catch (error) {
      throw error;
    }
  };

  const logout = () => {
    authService.logout();
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ user, login, register, logout, loading }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};
