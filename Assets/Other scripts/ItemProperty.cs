using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Item data
/// </summary>
public struct ItemProperty  
{
    /// <summary>
    /// Unique Id
    /// </summary>
    public int Id { get; private set; }
    /// <summary>
    /// Name (Alphanumeric)
    /// </summary>
    public string Name { get; private set; }
    /// <summary>
    /// Brief description
    /// </summary>
    public string Description { get; private set; }
}
