using System;
using Aertssen.Framework.Core.Builders;
using MyTemplate.Api.Contracts.Requests.MyEntity;

namespace MyTemplate.Tests.Helpers.T4 {
    public partial class StoreMyEntityRequestBuilder : ObjectBuilder<MyTemplate.Api.Contracts.Requests.MyEntity.StoreMyEntityRequest>
	{
       protected MyTemplate.Dto.MyEntity.StoreMyEntityDto MyEntity { get; set; }		
public StoreMyEntityRequestBuilder WithMyEntity(MyTemplate.Dto.MyEntity.StoreMyEntityDto @myentity)
{
    this.MyEntity = @myentity;
	return this;
}	

       
public override StoreMyEntityRequest Build()
{
    var item = (StoreMyEntityRequest)Activator.CreateInstance(typeof(StoreMyEntityRequest));
    	
		
	item.MyEntity = MyEntity;
	    
    return item;
}
      
    }
}

