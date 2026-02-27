import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export default function UserDetails() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <div style={styles.container}>
      <div style={styles.card}>

        {/* Avatar circle with initials */}
        <div style={styles.avatar}>
          {user?.firstName?.charAt(0).toUpperCase()}
          {user?.lastName?.charAt(0).toUpperCase()}
        </div>

        <h1 style={styles.title}>Welcome, {user?.firstName}!</h1>
        <p style={styles.subtitle}>Your account details</p>

        <div style={styles.detailsBox}>
          <div style={styles.detailRow}>
            <span style={styles.detailLabel}>First Name</span>
            <span style={styles.detailValue}>{user?.firstName}</span>
          </div>

          <div style={styles.divider} />

          <div style={styles.detailRow}>
            <span style={styles.detailLabel}>Last Name</span>
            <span style={styles.detailValue}>{user?.lastName}</span>
          </div>

          <div style={styles.divider} />

          <div style={styles.detailRow}>
            <span style={styles.detailLabel}>Email</span>
            <span style={styles.detailValue}>{user?.email}</span>
          </div>

          <div style={styles.divider} />

          <div style={styles.detailRow}>
            <span style={styles.detailLabel}>Member Since</span>
            <span style={styles.detailValue}>
              {new Date(user?.createdAt!).toLocaleDateString('en-ZA', {
                year: 'numeric',
                month: 'long',
                day: 'numeric',
              })}
            </span>
          </div>
        </div>

        <button onClick={handleLogout} style={styles.button}>
          Sign Out
        </button>
      </div>
    </div>
  );
}

const styles: { [key: string]: React.CSSProperties } = {
  container: {
    minHeight: '100vh',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    backgroundColor: '#f0f4f8',
    fontFamily: 'Arial, sans-serif',
  },
  card: {
    backgroundColor: '#ffffff',
    padding: '40px',
    borderRadius: '12px',
    boxShadow: '0 4px 20px rgba(0,0,0,0.1)',
    width: '100%',
    maxWidth: '460px',
    display: 'flex',
    flexDirection: 'column',
    alignItems: 'center',
  },
  avatar: {
    width: '72px',
    height: '72px',
    borderRadius: '50%',
    backgroundColor: '#2E86AB',
    color: '#fff',
    fontSize: '26px',
    fontWeight: 'bold',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    marginBottom: '16px',
    letterSpacing: '2px',
  },
  title: {
    margin: '0 0 8px 0',
    fontSize: '26px',
    fontWeight: 'bold',
    color: '#1E3A5F',
    textAlign: 'center',
  },
  subtitle: {
    margin: '0 0 28px 0',
    fontSize: '14px',
    color: '#777',
    textAlign: 'center',
  },
  detailsBox: {
    width: '100%',
    backgroundColor: '#f8fafc',
    borderRadius: '8px',
    border: '1px solid #e2e8f0',
    padding: '8px 0',
    marginBottom: '28px',
  },
  detailRow: {
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
    padding: '14px 20px',
  },
  detailLabel: {
    fontSize: '13px',
    fontWeight: '600',
    color: '#555',
  },
  detailValue: {
    fontSize: '14px',
    color: '#1E3A5F',
    fontWeight: '500',
  },
  divider: {
    height: '1px',
    backgroundColor: '#e2e8f0',
    margin: '0 20px',
  },
  button: {
    width: '100%',
    padding: '12px',
    backgroundColor: '#e74c3c',
    color: '#fff',
    border: 'none',
    borderRadius: '6px',
    fontSize: '16px',
    fontWeight: 'bold',
    cursor: 'pointer',
  },
};