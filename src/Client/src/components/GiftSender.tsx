import React, { useState } from 'react';
import { useGameContext } from '../context/GameContext';

const GiftSender: React.FC = () => {
    const { sendGift, player } = useGameContext();
    const [friendId, setFriendId] = useState<string>('');
    const [resourceType, setResourceType] = useState<number>(0);
    const [amount, setAmount] = useState<string>('5');

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        const numAmount = parseInt(amount, 10);
        if (!isNaN(numAmount)) {
            sendGift(friendId, resourceType, numAmount);
        }
    };

    if (!player) return null;

    return (
        <div className="gift-sender">
            <h3>Send Gift</h3>
            <form onSubmit={handleSubmit}>
                <div className="form-group">
                    <label>Friend's Player ID:</label>
                    <input
                        type="text"
                        value={friendId}
                        onChange={(e) => setFriendId(e.target.value)}
                        placeholder="Enter player ID"
                        required
                    />
                </div>
                <div className="form-group">
                    <label>Resource Type:</label>
                    <select
                        value={resourceType}
                        onChange={(e) => setResourceType(parseInt(e.target.value, 10))}
                    >
                        <option value={0}>Coins</option>
                        <option value={1}>Rolls</option>
                    </select>
                </div>
                <div className="form-group">
                    <label>Amount:</label>
                    <input
                        type="number"
                        value={amount}
                        onChange={(e) => setAmount(e.target.value)}
                        min="1"
                        max={resourceType === 0 ? player.coins : player.rolls}
                        required
                    />
                    <small>
                        Max: {resourceType === 0 ? player.coins : player.rolls}
                    </small>
                </div>
                <button type="submit" className="action-btn">
                    Send Gift
                </button>
            </form>
        </div>
    );
};

export default GiftSender;