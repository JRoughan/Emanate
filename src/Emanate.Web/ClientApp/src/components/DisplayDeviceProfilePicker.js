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
            <div>
                <h4>Device Profile</h4>
                {renderDisplayDeviceProfiles(this.props)}
            </div>
        );
    }
}

function renderDisplayDeviceProfiles(props) {
    return (
        <select>
            {props.displayDeviceProfiles.map(profile =>
                <option key={profile.id} value={profile.id}>{profile.name}</option>
        )}
        </select>
    );
}

export default connect(
    state => state.displayDeviceProfilePicker,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(DisplayDeviceProfilePicker);
