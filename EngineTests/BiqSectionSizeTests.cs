using System.Runtime.InteropServices;
using QueryCiv3;
using QueryCiv3.Biq;
using Xunit;

namespace EngineTests;

public class BiqSectionSizeTests {
	// The BIQ reader in QueryCiv3 splits dynamic sections into fixed-size chunks using the
	// [SECTION]_LEN_* constants; each constant is an offset bracket into the section's struct.
	// The sum of the constants must therefore equal the struct's size. A mismatch means a
	// Buffer.MemoryCopy can write past the end of the struct's backing memory. This happened
	// for CITY (38 + 36 = 74 vs. sizeof(CITY) = 70), writing 4 bytes past every city array
	// element and into the GC heap past the last one.
	[Fact]
	public void DynamicSectionLengthConstants_SumToStructSize() {
		Assert.Equal(Marshal.SizeOf<GOVT>(), BiqData.GOVT_LEN_1 + BiqData.GOVT_LEN_2);
		Assert.Equal(Marshal.SizeOf<TERR>(), BiqData.TERR_LEN_1 + BiqData.TERR_LEN_2);
		Assert.Equal(Marshal.SizeOf<RACE>(), BiqData.RACE_LEN_1 + BiqData.RACE_LEN_2 + BiqData.RACE_LEN_3 + BiqData.RACE_LEN_4);
		Assert.Equal(Marshal.SizeOf<CITY>(), BiqData.CITY_LEN_1 + BiqData.CITY_LEN_2);
		Assert.Equal(Marshal.SizeOf<WMAP>(), BiqData.WMAP_LEN_1 + BiqData.WMAP_LEN_2);
		Assert.Equal(Marshal.SizeOf<PRTO>(), BiqData.PRTO_LEN_1 + BiqData.PRTO_LEN_2);
		Assert.Equal(Marshal.SizeOf<LEAD>(), BiqData.LEAD_LEN_1 + BiqData.LEAD_LEN_2 + BiqData.LEAD_LEN_3);
		Assert.Equal(Marshal.SizeOf<RULE>(), BiqData.RULE_LEN_1 + BiqData.RULE_LEN_2 + BiqData.RULE_LEN_3);
		Assert.Equal(Marshal.SizeOf<GAME>(), BiqData.GAME_LEN_1 + BiqData.GAME_LEN_2 + BiqData.GAME_LEN_3);
	}
}
