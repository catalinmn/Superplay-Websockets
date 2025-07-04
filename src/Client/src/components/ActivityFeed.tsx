import React from 'react';
import { useGameContext } from '../context/GameContext';

const ActivityFeed: React.FC = () => {
  const { activityLog } = useGameContext();

  return (
    <div className="activity-feed">
      <h3>Activity Log</h3>
      <div className="log-container">
        {activityLog.length === 0 ? (
          <div className="empty-log">No activity yet</div>
        ) : (
          [...activityLog].reverse().map((log, index) => (
            <div key={`${log.timestamp}-${index}`} className="log-entry">
              <span className="timestamp">
                {new Date(log.timestamp).toLocaleTimeString()}
              </span>
              <span className="message">{log.text}</span>
            </div>
          ))
        )}
      </div>
    </div>
  );
};

export default ActivityFeed;