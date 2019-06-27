import React, { Component } from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { actionCreators } from '../store/Admin';

class Admin extends Component {
    componentDidMount() {
        // This method is called when the component is first added to the document
        this.ensureDataFetched();
    }

    ensureDataFetched() {
        this.props.requestDisplayDevices();
    }

    render() {
        return (
            <div>
                <h1>Admin</h1>
                <h2>Display Devices</h2>
                {renderDisplayDevices(this.props)}
            </div>
        );
    }
}

function renderDisplayDevices(props) {
    return (
        <ul>
            {props.displayDevices.map(device =>
                <li key={device.id}>{device.name}</li>
            )}
        </ul>
    );
}

export default connect(
    state => state.admin,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(Admin);
