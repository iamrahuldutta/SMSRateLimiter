import React from "react";

const SmsLogsTable = ({ logs }) => (
    <table className="table table-striped table-bordered">
        <thead className="table-dark">
            <tr>
                <th>Account ID</th>
                <th>Phone Number</th>
                <th>Timestamp</th>
            </tr>
        </thead>
        <tbody>
            {logs.length === 0 ? (
                <tr>
                    <td colSpan="3" className="text-center">No logs found.</td>
                </tr>
            ) : (
                logs.map((log) => (
                    <tr key={log.id}>
                        <td>{log.accountId}</td>
                        <td>{log.phoneNumber}</td>
                        <td>{new Date(log.timestamp).toLocaleString()}</td>
                    </tr>
                ))
            )}
        </tbody>
    </table>
);

export default SmsLogsTable;