struct AssembledVertex
{
    float3 inPosition : POSITION;
    float3 inColor : COLOR;
};

struct FinalVertex
{
    float3 fragColor;
};

struct VertexStageOutput
{
    FinalVertex finalVertex : FinalVertex;
    float4 sv_position : SV_Position;
};

[[vk::binding(0)]]
cbuffer SceneUniforms
{
    float4x4 rotationMat;
};


[shader("vertex")]
VertexStageOutput vertexMain(
    AssembledVertex assembledVertex)
{
    VertexStageOutput vstageOutput;
    vstageOutput.sv_position = mul(float4(assembledVertex.inPosition, 1.0f), rotationMat);
    vstageOutput.finalVertex.fragColor = assembledVertex.inColor;
    return vstageOutput;
}


struct Fragment
{
    float4 color;
};

[shader("fragment")]
Fragment fragmentMain(
    FinalVertex finalVertex: FinalVertex) : SV_Target
{
    Fragment fragOut;
    fragOut.color = float4(finalVertex.fragColor, 1.0f);

    return fragOut;
}