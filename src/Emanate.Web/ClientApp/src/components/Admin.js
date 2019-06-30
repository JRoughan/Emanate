import React, { Component } from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { actionCreators } from '../store/Admin';
import DisplayDeviceGroup from './DisplayDeviceGroup';

class Admin extends Component {

    render() {
        return (
            <div>
                <h1>Admin</h1>
                <DisplayDeviceGroup displayDevices={this.props.displayDevices} />
            </div>
        );
    }
}

export default connect(
    state => state.admin,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(Admin);
