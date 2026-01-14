import api from './api';

export const apiService = {
  // Plans
  async getPlans() {
    const response = await api.get('/plans');
    return response.data;
  },

  async getPlan(id) {
    const response = await api.get(`/plans/${id}`);
    return response.data;
  },

  // Users
  async getUser(id) {
    const response = await api.get(`/users/${id}`);
    return response.data;
  },

  async updateUser(id, data) {
    const response = await api.put('/users', { id, ...data });
    return response.data;
  },

  // Subscriptions
  async getSubscriptions() {
    const response = await api.get('/subscriptions');
    return response.data;
  },

  async createSubscription(userId, planId, startDate) {
    const response = await api.post('/subscriptions', {
      userId,
      planId,
      startDate: startDate.toISOString().split('T')[0],
    });
    return response.data;
  },

  // Password Check
  async checkPassword(password) {
    const response = await api.post('/passwordcheck/check', { password });
    return response.data;
  },
};
