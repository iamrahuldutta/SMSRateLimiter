import React from "react";

const FiltersForm = ({ filters, setFilters, onSubmit }) => (
    <form
        onSubmit={(e) => {
            e.preventDefault();
            onSubmit();
        }}
        className="row g-3 mb-4"
    >
        <div className="col-md-2">
            <input
                type="number"
                placeholder="Account ID"
                value={filters.accountId}
                onChange={(e) => setFilters((prev) => ({ ...prev, accountId: e.target.value }))}
                required
                className="form-control"
            />
        </div>
        <div className="col-md-3">
            <input
                type="text"
                placeholder="Phone Number"
                value={filters.phoneNumber}
                onChange={(e) => setFilters((prev) => ({ ...prev, phoneNumber: e.target.value }))}
                className="form-control"
            />
        </div>
        <div className="col-md-3">
            <input
                type="datetime-local"
                onChange={(e) => setFilters((prev) => ({ ...prev, from: e.target.value }))}
                className="form-control"
                placeholder="From"
            />
        </div>
        <div className="col-md-3">
            <input
                type="datetime-local"
                onChange={(e) => setFilters((prev) => ({ ...prev, to: e.target.value }))}
                className="form-control"
                placeholder="To"
            />
        </div>
        <div className="col-md-1">
            <button type="submit" className="btn btn-primary w-100">
                Search
            </button>
        </div>
    </form>
);

export default FiltersForm;