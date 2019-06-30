import React, { Component } from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { actionCreators } from '../store/DisplayDeviceProfilePicker';

class DisplayDeviceProfilePicker extends Component {

    changeProfile = (event) => {
        if (event.target.value === "Manage") {
            console.log("Manage profiles");
        } else {
            this.props.setDisplayDeviceProfile(this.props.device, event.target.value);
        }
    }

    render() {
        return (
            <select value={this.props.device.profileId} onChange={this.changeProfile}>
                <option hidden>Select Profile</option>
                {this.props.displayDeviceProfiles.map(profile =>
                    <option key={profile.id} value={profile.id}>{profile.name}</option>
                )}
                <option value="Manage">&lt;Manage...&gt;</option>
            </select>
        );
    }
}

export default connect(
    state => state || {},
    dispatch => bindActionCreators(actionCreators, dispatch)
)(DisplayDeviceProfilePicker);
