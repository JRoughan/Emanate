import React from 'react';
import OutputGroup from './OutputGroup.jsx';

export default ({outputGroups}) => {
  return (
    <div className="outputGroups">{outputGroups.map(outputGroup =>
      <OutputGroup className="outputGroup" key={outputGroup.id} outputGroup={outputGroup} />
    )}</div>
  );
}