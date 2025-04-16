'use client';

import { useState, useEffect } from 'react';
import { faClock } from '@fortawesome/free-solid-svg-icons';
import { faCalendarDays } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

const RealTimeClock = () => {
    const [time, setTime] = useState<Date | null>(null);
    const [mounted, setMounted] = useState(false);

    useEffect(() => {
        setMounted(true);
        const updateTime = () => {
            const now = new Date();
            setTime(now);
        };

        updateTime(); // Update immediately on mount
        const interval = setInterval(updateTime, 1000); // Update every second

        return () => clearInterval(interval); // Cleanup interval on unmount
    }, []);

    if (!mounted) {
        return null; // Don't render anything on the server
    }

    return (
        <div className="text-xl font-bold text-white ml-auto">
            <p>
                <FontAwesomeIcon icon={faCalendarDays} className="mr-3" />
                {time?.toLocaleDateString('vi-VN')}
            </p>
            <p>
                <FontAwesomeIcon icon={faClock} className="mr-3" />
                {time?.toLocaleTimeString()}
            </p>
        </div>
    );
};

export default RealTimeClock;
