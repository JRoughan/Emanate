import React, { Component } from 'react';
import { connect } from 'react-redux';

class DisplayConfigurationsGroup extends Component {

    render() {
        return (
            <div>
                <h5>Source Configurations</h5>
                {this.props.sourceConfigurations.map(config =>
                    <div key={config.id} >
                        {config.builds}
                    </div>
                )}
            </div>
        );
    }
}

export default connect()(DisplayConfigurationsGroup);

