import React, { Component } from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { actionCreators } from '../store/Dashboard';
import DisplayConfigurationsGroup from './DisplayConfigurationsGroup';

class Dashboard extends Component {

    componentDidMount() {
        this.props.requestDisplayConfigurations();
    }

    render() {
        return (
            <div>
                <h1>Dashboard</h1>
                <DisplayConfigurationsGroup displayConfigurations={this.props.displayConfigurations} />
            </div>
        );
    }
}

export default connect(
    state => state.dashboard,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(Dashboard);
