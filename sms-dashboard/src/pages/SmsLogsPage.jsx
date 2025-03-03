import React, { useState, useEffect } from "react";
import SmsLogsTable from "../components/SmsLogsTable";
import FiltersForm from "../components/FiltersForm";
import { fetchSmsLogs } from "../services/smsApi";

const SmsLogsPage = () => {
    const [logs, setLogs] = useState([]);
    const [filters, setFilters] = useState({ accountId: "" });
    const [loading, setLoading] = useState(false);

    const loadLogs = async () => {
        setLoading(true);
        try {
            const logsData = await fetchSmsLogs(filters);
            setLogs(logsData);
        } catch (error) {
            console.error("Failed to fetch SMS logs", error);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        if (filters.accountId) {
            loadLogs();
        }
    }, []);

    return (
        <div className="container py-5">
            <h1 className="mb-4">SMS Logs</h1>
            <FiltersForm filters={filters} setFilters={setFilters} onSubmit={loadLogs} />
            {loading ? (
                <div className="text-center mt-5">
                    <div className="spinner-border text-primary" role="status">
                        <span className="visually-hidden">Loading...</span>
                    </div>
                </div>
            ) : (
                <SmsLogsTable logs={logs} />
            )}
        </div>
    );
};

export default SmsLogsPage;