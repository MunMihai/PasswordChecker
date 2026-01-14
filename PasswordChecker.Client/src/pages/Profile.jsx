import { useState, useEffect } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { apiService } from '../services/apiService';
import ProtectedRoute from '../components/ProtectedRoute';

function ProfileContent() {
  const { user } = useAuth();
  const [userData, setUserData] = useState(null);
  const [subscriptions, setSubscriptions] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      const [userInfo, subs] = await Promise.all([
        apiService.getUser(user.id),
        apiService.getSubscriptions(),
      ]);
      setUserData(userInfo);
      setSubscriptions(subs.filter(s => s.userEmail === user.email));
    } catch (err) {
      console.error('Failed to load data', err);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
        <div className="text-center">Loading...</div>
      </div>
    );
  }

  const activeSubscription = subscriptions.find(s => s.status === 'ACTIVE');

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
      <h1 className="text-3xl font-bold text-gray-900 mb-8">Profile</h1>

      <div className="bg-white shadow rounded-lg p-6 mb-6">
        <h2 className="text-xl font-semibold mb-4">User Information</h2>
        <dl className="grid grid-cols-1 gap-4 sm:grid-cols-2">
          <div>
            <dt className="text-sm font-medium text-gray-500">Email</dt>
            <dd className="mt-1 text-sm text-gray-900">{userData?.email}</dd>
          </div>
          <div>
            <dt className="text-sm font-medium text-gray-500">Role</dt>
            <dd className="mt-1 text-sm text-gray-900">{userData?.role}</dd>
          </div>
          <div>
            <dt className="text-sm font-medium text-gray-500">Balance</dt>
            <dd className="mt-1 text-sm text-gray-900">${userData?.balance?.toFixed(2) || '0.00'}</dd>
          </div>
          {userData?.varsta && (
            <div>
              <dt className="text-sm font-medium text-gray-500">Age</dt>
              <dd className="mt-1 text-sm text-gray-900">{userData.varsta}</dd>
            </div>
          )}
        </dl>
      </div>

      <div className="bg-white shadow rounded-lg p-6">
        <h2 className="text-xl font-semibold mb-4">Subscription</h2>
        {activeSubscription ? (
          <div>
            <p className="text-sm text-gray-600 mb-2">
              <span className="font-medium">Plan:</span> {activeSubscription.planName}
            </p>
            <p className="text-sm text-gray-600 mb-2">
              <span className="font-medium">Status:</span>{' '}
              <span className={`inline-flex px-2 py-1 text-xs font-semibold rounded-full ${
                activeSubscription.status === 'ACTIVE' ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-800'
              }`}>
                {activeSubscription.status}
              </span>
            </p>
            <p className="text-sm text-gray-600">
              <span className="font-medium">Start Date:</span> {activeSubscription.startDate}
            </p>
            {activeSubscription.endDate && (
              <p className="text-sm text-gray-600">
                <span className="font-medium">End Date:</span> {activeSubscription.endDate}
              </p>
            )}
          </div>
        ) : (
          <div>
            <p className="text-gray-600 mb-4">No active subscription</p>
            <a
              href="/pricing"
              className="inline-block bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-md text-sm font-medium"
            >
              View Plans
            </a>
          </div>
        )}
      </div>
    </div>
  );
}

export default function Profile() {
  return (
    <ProtectedRoute>
      <ProfileContent />
    </ProtectedRoute>
  );
}
