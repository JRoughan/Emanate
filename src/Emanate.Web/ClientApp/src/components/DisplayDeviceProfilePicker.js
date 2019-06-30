import React, { Component } from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { actionCreators } from '../store/DisplayDeviceProfilePicker';

class DisplayDeviceProfilePicker extends Component {

    componentDidMount() {
        this.props.requestDisplayDeviceProfiles();
    }

    render() {
        return (
            <select value={this.props.currentProfileId}>
                {this.props.displayDeviceProfiles.map(profile =>
                    <option key={profile.id} value={profile.id}>{profile.name}</option>
                )}
            </select>
        );
    }
}

export default connect(
    state => state.displayDeviceProfilePicker,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(DisplayDeviceProfilePicker);
