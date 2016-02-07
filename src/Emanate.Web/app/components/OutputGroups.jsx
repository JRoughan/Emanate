import React from 'react';
import OutputGroup from './OutputGroup.jsx';

export default ({outputGroups, onEdit, onDelete}) => {
    return (
      <ul className="outputGroups">{outputGroups.map(outputGroup =>
          <li className="outputGroup" key={outputGroup.id}>
            <OutputGroup
              name={outputGroup.name}
              onEdit={onEdit.bind(null, outputGroup.id)}
              onDelete={onDelete.bind(null, outputGroup.id)} />
          </li>
    )}</ul>
  );
}
