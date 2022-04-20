using System;
using Aertssen.Framework.Core.Builders;
using MyTemplate.Domain.Model;

namespace MyTemplate.Domain.T4 {
    public partial class MyEntityBuilder : ObjectBuilder<MyTemplate.Domain.Model.MyEntity>
	{
       protected System.String Name { get; set; }		
public MyEntityBuilder WithName(System.String @name)
{
    this.Name = @name;
	return this;
}	
protected System.String CreatedBy { get; set; }		
public MyEntityBuilder WithCreatedBy(System.String @createdby)
{
    this.CreatedBy = @createdby;
	return this;
}	
protected System.DateTimeOffset CreatedOn { get; set; }		
public MyEntityBuilder WithCreatedOn(System.DateTimeOffset @createdon)
{
    this.CreatedOn = @createdon;
	return this;
}	
protected System.String ModifiedBy { get; set; }		
public MyEntityBuilder WithModifiedBy(System.String @modifiedby)
{
    this.ModifiedBy = @modifiedby;
	return this;
}	
protected System.DateTimeOffset ModifiedOn { get; set; }		
public MyEntityBuilder WithModifiedOn(System.DateTimeOffset @modifiedon)
{
    this.ModifiedOn = @modifiedon;
	return this;
}	
protected System.Int64 Id { get; set; }		
public MyEntityBuilder WithId(System.Int64 @id)
{
    this.Id = @id;
	return this;
}	

       
public override MyEntity Build()
{
    var item = (MyEntity)Activator.CreateInstance(typeof(MyEntity));
    	
		
	item.Name = Name;
	    	
		
	item.CreatedBy = CreatedBy;
	    	
		
	item.CreatedOn = CreatedOn;
	    	
		
	item.ModifiedBy = ModifiedBy;
	    	
		
	item.ModifiedOn = ModifiedOn;
	    	
		
	item.Id = Id;
	    
    return item;
}
      
    }
}

