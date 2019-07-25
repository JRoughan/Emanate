import React, { Component } from 'react';
import { connect } from 'react-redux';

class DisplayConfigurationsGroup extends Component {

    render() {
        return (
            <div>
                <h2>Display Configurations</h2>
                <div className="card-group">
                    {this.props.displayConfigurations.map(config =>
                        <div className="card" key={config.id} >
                            <div className="card-header">
                                <h3 className="card-title">{config.displayDevice.name}</h3>
                            </div>
                            <div className="card-body">
                                {config.sourceGroups.map(sourceGroup => {
                                    return (
                                        <div>
                                            <h4>{sourceGroup.sourceDevice.name}</h4>
                                            {sourceGroup.sourceConfiguration.builds}
                                        </div>
                                    );
                                })}
                            </div>
                            <div className="card-footer">
                                <small className="text-muted">Active</small>
                            </div>
                        </div>
                    )}
                </div>
            </div>
        );
    }
}

export default connect()(DisplayConfigurationsGroup);

